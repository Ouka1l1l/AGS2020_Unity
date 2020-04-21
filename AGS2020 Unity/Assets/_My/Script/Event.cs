using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
    public enum EventType
    {
        Non,
        Stairs,
        Max
    }

    public EventType _type { get; protected set; }

    public abstract void Raise(Character character);
}
