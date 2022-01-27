using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract BodySystem bodySystem { get; set; }
}
