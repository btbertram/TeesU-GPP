using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Data.Common;
using Mono.Data.Sqlite;
using System.Security.Cryptography;
/// <summary>
/// A Connection Class which communicates with the database for account related queries and modifications.
/// </summary>
public class AccountConnection : MonoBehaviour
{

    /// <summary>
    /// Inserts a new User Account into the SQLite database.
    /// Uses SQL parameters, guid salts, and hash for basic security.
    /// </summary>
    /// <param name="newUsername">The user supplied name for the new account.</param>
    /// <param name="newPasscode">The user supplied passcode for the new account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    public async Task<BoolStringResult> CreateAccountAsync(string newUsername, string newPasscode)
    {
        BoolStringResult result;
        result = InputQuickExit(newUsername, newPasscode);
        if (!result._successful)
        {
            return result;
        }

        result = await TestUsernameAvailabilityAsync(newUsername);

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

        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();

        ConnectionManager.CreateNamedParamater("@newUsername", newUsername, dbCommand);
        ConnectionManager.CreateNamedParamater("@finalHash", finalHash, dbCommand);
        ConnectionManager.CreateNamedParamater("@guid", guid.ToString(), dbCommand);
        dbCommand.CommandText = insertQuery;

        ///ExecuteNonQuery returns # of rows affected by command when-
        ///command is a UPDATE, INSERT, or DELETE.
        ///-1 is returned when any other command is used.
        int returnVal = await dbCommand.ExecuteNonQueryAsync();

        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();

        if (returnVal == 1)
        {

            //Check if newly created account works
            Task<BoolStringResult> verifyTask = VerifyAccountAsync(newUsername, newPasscode);
            result = await verifyTask;

            Debug.Log("Verified");

            if (result._successful)
            {
                //We add new UserStat row based on user/ID

                string selectQuery = "SELECT ID FROM UserAccounts WHERE username = @username;";

                //ConnectionManager.OpenInstanceConnection();
                DbCommand dbCommandAccountSetup = ConnectionManager.GetConnection().CreateCommand();

                ConnectionManager.CreateNamedParamater("@username", newUsername, dbCommandAccountSetup);
                dbCommandAccountSetup.CommandText = selectQuery;
                Task<DbDataReader> readerTask = dbCommandAccountSetup.ExecuteReaderAsync();
                int uid = -1;

                DbDataReader reader = await readerTask;

                while (reader.Read())
                {
                    uid = reader.GetInt32(0);
                }
                reader.Close();
                reader.Dispose();            

                ConnectionManager.CreateNamedParamater("@uid", uid, dbCommandAccountSetup);

                Debug.Log("Adding new user info");
                Debug.Log(uid);

                insertQuery = "INSERT into UserStats(userID, username) VALUES(@uid, @username);";
                dbCommandAccountSetup.CommandText = insertQuery;
                await Task.Run(() => dbCommandAccountSetup.ExecuteNonQueryAsync());

                insertQuery = "INSERT into PlayerStatus(playerID) VALUES(@uid);";
                dbCommandAccountSetup.CommandText = insertQuery;
                await Task.Run(() => dbCommandAccountSetup.ExecuteNonQueryAsync());

                for (int x = 0; x < (int)EAchievements.Error; x++)
                {
                    int achieveID = x;
                    insertQuery = "INSERT into PlayerAchievements(playerID, achievementID, unlocked) VALUES(@uid, " + x + ", 0);";
                    dbCommandAccountSetup.CommandText = insertQuery;
                    await Task.Run(() => dbCommandAccountSetup.ExecuteNonQueryAsync());
                }

                dbCommandAccountSetup.Dispose();

                //ConnectionManager.CloseInstanceConnection();

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
    }

    /// <summary>
    /// Attempts to verify a user account by testing provided information against information in the database,
    /// using hash comparisons. Currently reads out debug logs in unity for results.
    /// </summary>
    /// <param name="username">A user provided account name.</param>
    /// <param name="passcode">A user provided passcode for the account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    /// <returns>True if the information provided matches the database information, false if it does not.</returns>
    public async Task<BoolStringResult> VerifyAccountAsync(string username, string passcode)
    {
        BoolStringResult result;

        result = InputQuickExit(username, passcode);
        if (!result._successful)
        {
            return result;
        }

        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string selectQuerySaltHash = "SELECT salt, hash FROM UserAccounts WHERE username = @username;";
        ConnectionManager.CreateNamedParamater("@username", username, dbCommand);
        dbCommand.CommandText = selectQuerySaltHash;

        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        
        SHA256 sHA256 = SHA256.Create();
        string salt = "";
        string hash = "";

        DbDataReader reader = await readerTask;  

        while (reader.Read())
        {
            //salt = ByteArrayToString(dataReader.GetValue(0) as byte[]);
            //hash = ByteArrayToString(dataReader.GetValue(1) as byte[]);
            byte[] salttemp = reader.GetValue(0) as byte[];
            byte[] hashtemp = reader.GetValue(1) as byte[];


            salt = System.Text.Encoding.ASCII.GetString(salttemp);
            hash = System.Text.Encoding.ASCII.GetString(hashtemp);

        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();

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

    private async Task<BoolStringResult> TestUsernameAvailabilityAsync(string username)
    {
        BoolStringResult result;

        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        ConnectionManager.CreateNamedParamater("@username", username, dbCommand);

        string query = "SELECT username FROM UserAccounts WHERE username = @username;";
        dbCommand.CommandText = query;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();

        DbDataReader reader = await readerTask;

        if (reader.Read())
        {
            reader.Close();
            reader.Dispose();
            dbCommand.Dispose();

            //ConnectionManager.CloseInstanceConnection();

            //if there's anything to read, we have a matching username, so...
            result._stringMessage = "Username in use.";
            result._successful = false;
            return result;
        }
        else
        {
            reader.Close();
            reader.Dispose();
            dbCommand.Dispose();

            //ConnectionManager.CloseInstanceConnection();

            //Otherwise, there are no results, which mean no matches for that username.
            //The provided username is available, so...
            result._stringMessage = "";
            result._successful = true;
            return result;
        }
    }

    public async Task GrantAuthAsync(bool verified, string username)
    {
        if (verified)
        {
            string selectQueryID = "SELECT ID FROM UserAccounts WHERE username = @username;";

            //ConnectionManager.OpenInstanceConnection();

            DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
            ConnectionManager.CreateNamedParamater("@username", username, dbCommand);
            dbCommand.CommandText = selectQueryID;
            Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
            DbDataReader reader = await readerTask;

            //Temp assigned -1 to prevent data collision
            int tempID = -1;

            while (reader.Read())
            {
                tempID = reader.GetInt32(0);
            }
            reader.Close();
            reader.Dispose();

            dbCommand.Dispose();

            //ConnectionManager.CloseInstanceConnection();

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
