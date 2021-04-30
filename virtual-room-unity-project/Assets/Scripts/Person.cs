using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour {
    public virtual string Id { get; set; }
    public static List<Person> AllPeople { get; } = new List<Person>();

    protected virtual void Start() {
        AllPeople.Add(this);
    }

    protected virtual void OnDestroy() {
        AllPeople.Remove(this);
    }
}
