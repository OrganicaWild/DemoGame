using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TextInput : MonoBehaviour
{
    private Action typed;
    private InputField field;

    // Start is called before the first frame update
    void Start()
    {
        field = GetComponent<InputField>();
        Keyboard.current.onTextInput += textInput;
    }

    private void OnDestroy()
    {
        Keyboard.current.onTextInput -= textInput;
    }
    
    private void textInput(char c)
    {
        Debug.Log(c);
        field.text += c;
    }
    
}