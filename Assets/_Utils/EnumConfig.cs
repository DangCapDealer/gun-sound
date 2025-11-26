using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumConfig
{

}

public enum CharacterType
{
    None,
    Melee,
    Archer
}

public enum CharacterTeam
{
    None,
    Ally,
    Enemy
}

public enum CharacterSkill
{
    None,
    Poisoned,
    Critical,
    Stun,
    Freeze,
    Health
}

public enum CharacterAnimatorState
{
    None,
    Initialized,
    Idle,
    Move,
    Attack,
    Die,
    Dancing
}


public enum GameState
{
    None,
    Home,
    Game,
    Pause,
    Win,
    Lose,
    Map,
}

public enum Popup
{
    None,
    Win,
    Lose,
    Setting,
    Character,
    Chapter,
    NewCharacter,
    Subscribe
}