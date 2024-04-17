using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : Singleton<MouseCursor>
{
    void Start()
    {
        // Hide the default cursor
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Updates the position of the sprite with the mouse position
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
    }
}