using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Animal 
{
    public string Name { get; set; }

    public void Eat()
    {
        Debug.Log(Name + "is eating");
    }
}

class Dog : Animal
{
    public void Bark()
    {
        Debug.Log(Name + "is barking");
    }
}

class A
{
    public A()
    {
        B b = new B(Run);
    }

    public void Run()
    {
        Debug.Log("Run!!");
    }
}

class B
{
    public delegate void RunDelegate();
    public RunDelegate AfterRun;

    public B(RunDelegate afterRun)
    {
        AfterRun = afterRun;
    }

    public void Run()
    {
        Debug.Log("Run!!");
        AfterRun();
    }
}

public class MyGameManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject popupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Dog dog = new Dog();
        // dog.Name = "Bark";
        // dog.Eat();
        // dog.Bark();
        ShowPopup("HEELO", () =>
        {
            Debug.Log("Show popup");
        });
    }

    // Update is called once per frame
    public void ShowPopup(string message, System.Action onConfirm = null)
    {
        var popupObject = Instantiate(popupPrefab, canvas.transform);
        var popup = popupObject.GetComponent<MyPopupPanelController>();
        popup.Setup(message, onConfirm);
    }

}
