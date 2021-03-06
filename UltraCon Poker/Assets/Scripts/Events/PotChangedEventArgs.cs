using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PotChangedEventArgs : EventArgs
{
    public int _pot {get; private set;}

    public PotChangedEventArgs(int pot)
    {
        _pot = pot;
    }
}
