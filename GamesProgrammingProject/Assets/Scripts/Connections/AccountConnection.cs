using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Security.Cryptography;

public class AccountConnection : MonoBehaviour
{

    #region Async Wrappers
    public async Task<BoolStringResult> CreateAccountAsync(string username, string passcode)
    {
        //Debug: Wait for two seconds
        await Task.Delay(2000);

        var result = await CreateAccount(username, passcode);

        return result;
    }

    public async Task<BoolStringResult> VerifyAccountAsync(string username, string passcode)
    {
        //Debug: Wait for five seconds
        await Task.Delay(5000);
        var result = await Task.FromResult<BoolStringResult>(VerifyAccount(username, passcode));

        return result;
    }

    #endregion

    /// <summary>
    /// Inserts a new User Account into the SQLite database.
    /// Uses SQL parameters, guid salts, and hash for basic security.
    /// </summary>
    /// <param name="newUsername">The user supplied name for the new account.</param>
    /// <param name="newPasscode">The user supplied passcode for the new account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    public async Task<BoolStringResult> CreateAccount(string newUsername, string newPasscode)
    {
        BoolStringResult result;

        //Test username
        result = InputQuickExit(newUsername, newPasscode);
        if (!result._successful)
        {
            return result;
        }

        result = await Task.FromResult<BoolStringResult>(TestUsernameAvailability(newUsername));
        if (!result._successful)
        {
            return result;
        }

        //Generate Salt
        System.Guid guid = System.Guid.NewGuid();

        //Combine salt with newPasscode
        //ASCII works with db, unicode does not? Experiment.
        byte[] encodedPasscode = System.Text.Encoding.ASCII.GetBytes(newPasscode + guid + newUsername);

        //Hash Salted Passcode
        ///Create Hashgen

        SHA256 sHA256 = SHA256.Create();

        byte[] computedHash = sHA256.ComputeHash(encodedPasscode);

        string finalHash = ConnectionManager.ByteArrayContentsToString(computedHash);

        //Insert query to database - new entry in user account table
        ///Sends User and Hash
        ///Need Database connection to do this - Should have a class that saves connection. Scriptable object?

        string insertQuery = "INSERT into UserAccounts(username, hash, salt) VALUES(@newUsername, @finalHash, @guid);";
        IDbCommand dbCommand = ConnectionManager.GetCMInstance()._dbConnection.CreateCommand();

        ConnectionManager.CreateNamedParamater("@newUsername", newUsername, dbCommand);
        ConnectionManager.CreateNamedParamater("@finalHash", finalHash, dbCommand);
        ConnectionManager.CreateNamedParamater("@guid", guid.ToString(), dbCommand);
        dbCommand.CommandText = insertQuery;

        ///ExecuteNonQuery returns # of rows affected by command when-
        ///command is a UPDATE, INSERT, or DELETE.
        ///-1 is returned when any other command is used.
        int returnVal = dbCommand.ExecuteNonQuery();
        Debug.Log("Attempted Account creation");
        if (returnVal == 1)
        {
            //Check if newly created account works
            result = await VerifyAccountAsync(newUsername, newPasscode);
            if (result._successful)
            {
                //We add new UserStat row based on user/ID
                dbCommand.Parameters.Clear();

                string selectQuery = "SELECT ID FROM UserAccounts WHERE username = @username;";
                ConnectionManager.CreateNamedParamater("@username", newUsername, dbCommand);
                dbCommand.CommandText = selectQuery;
                int id = -1;

                IDataReader reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }
                reader.Close();
                reader.Dispose();

                ConnectionManager.CreateNamedParamater("@ID", id, dbCommand);

                insertQuery = "INSERT into UserStats(userID, username) VALUES(@ID, @username);";

                dbCommand.CommandText = insertQuery;
                dbCommand.ExecuteNonQuery();

                result._stringMessage = "Account Created!";



                return result;
            }
            else
            {
                result._stringMessage = "Unable to verify account creation.";
                return result;
            }

        }
        else
        {
            result._successful = false;
            result._stringMessage = "Error during account creation.";
            return result;
        }
        //DB side - Trigger after insert: Create new User data table
    }

    /// <summary>
    /// Attempts to verify a user account by testing provided information against information in the database,
    /// using hash comparisons. Currently reads out debug logs in unity for results.
    /// </summary>
    /// <param name="username">A user provided account name.</param>
    /// <param name="passcode">A user provided passcode for the account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    /// <returns>True if the information provided matches the database information, false if it does not.</returns>
    public BoolStringResult VerifyAccount(string username, string passcode)
    {
        BoolStringResult result;

        result = InputQuickExit(username, passcode);
        if (!result._successful)
        {
            return result;
        }


        SHA256 sHA256 = SHA256.Create();
        string selectQuerySaltHash = "SELECT salt, hash FROM UserAccounts WHERE username = @username;";
        IDbCommand dbCommand = ConnectionManager.GetCMInstance()._dbConnection.CreateCommand();
        ConnectionManager.CreateNamedParamater("@username", username, dbCommand);
        dbCommand.CommandText = selectQuerySaltHash;
        IDataReader dataReader = dbCommand.ExecuteReader();
        string salt = "";
        string hash = "";

        while (dataReader.Read())
        {
            //salt = ByteArrayToString(dataReader.GetValue(0) as byte[]);
            //hash = ByteArrayToString(dataReader.GetValue(1) as byte[]);
            byte[] salttemp = dataReader.GetValue(0) as byte[];
            byte[] hashtemp = dataReader.GetValue(1) as byte[];


            salt = System.Text.Encoding.ASCII.GetString(salttemp);
            hash = System.Text.Encoding.ASCII.GetString(hashtemp);

            //Debug.Log("TestHold");
        }
        dataReader.Close();

        byte[] encodedPasscode = System.Text.Encoding.ASCII.GetBytes(passcode + salt + username);
        byte[] computedHash = sHA256.ComputeHash(encodedPasscode);

        if (hash == ConnectionManager.ByteArrayContentsToString(computedHash))
        {
            Debug.Log("Salt from db is:" + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ConnectionManager.ByteArrayContentsToString(computedHash));
            result._successful = true;
            result._stringMessage = "";
            return result;
        }
        else
        {
            Debug.Log("Salt from db is: " + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ConnectionManager.ByteArrayContentsToString(computedHash));
            result._successful = false;
            result._stringMessage = "Invalid Username or Password.";
            return result;

        }
    }

    private BoolStringResult InputQuickExit(string username, string passcode)
    {
        BoolStringResult result;

        if (username == "")
        {
            result._successful = false;
            result._stringMessage = "No Username Given. Please enter a username.";

            return result;
        }

        if (passcode == "")
        {
            result._successful = false;
            result._stringMessage = "No Password Given. Please enter a password.";

            return result;
        }

        result._stringMessage = "";
        result._successful = true;
        return result;
    }

    private BoolStringResult TestUsernameAvailability(string username)
    {
        BoolStringResult result;

        IDbCommand dbCommand = ConnectionManager.GetCMInstance()._dbConnection.CreateCommand();
        ConnectionManager.CreateNamedParamater("@username", username, dbCommand);

        string query = "SELECT username FROM UserAccounts WHERE username = @username;";
        dbCommand.CommandText = query;

        IDataReader dataReader = dbCommand.ExecuteReader();

        if (dataReader.Read())
        {
            //if there's anything to read, we have a matching username, so...
            result._stringMessage = "Username in use.";
            result._successful = false;
            return result;
        }
        else
        {
            //Otherwise, there are no results, which mean no matches for that username.
            //The provided username is available, so...
            result._stringMessage = "";
            result._successful = true;
            return result;
        }
    }

    public void GrantAuth(bool verified, string username)
    {
        if (verified)
        {
            string selectQueryID = "SELECT ID FROM UserAccounts WHERE username = @username;";
            IDbCommand dbCommand = ConnectionManager.GetCMInstance()._dbConnection.CreateCommand();
            ConnectionManager.CreateNamedParamater("@username", username, dbCommand);
            dbCommand.CommandText = selectQueryID;
            IDataReader dataReader = dbCommand.ExecuteReader();
            //Temp assigned -1 to prevent data collision
            int tempID = -1;

            while (dataReader.Read())
            {
                tempID = dataReader.GetInt32(0);
            }
            dataReader.Close();

            //Create "AuthToken"
            UserSessionManager.CreateUserSessionInstance(tempID, username);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.GetCMInstance();
    }

}
