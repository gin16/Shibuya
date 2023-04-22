using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

enum LocalButtons
{
    ClickLeft = 0,
    ClickRight = 1,
    Space = 2,
    Return = 3,
}
public struct LocalInput : INetworkInput
{
    public float HorizontalInput;
    public float VerticalInput;
    public NetworkButtons Buttons;
}