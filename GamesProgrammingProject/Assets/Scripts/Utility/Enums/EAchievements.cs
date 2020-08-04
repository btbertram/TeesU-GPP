using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An Enumerator for the game's list of achievements.
/// Note that the order here lines up with the achievementID in the Achievements table in the database.
/// </summary>
public enum EAchievements
{
    TotalGathers,
    DistanceTraveled,
    TotalGold,
    Error
}
