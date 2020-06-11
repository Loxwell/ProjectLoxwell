
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BitField = LSG.Utilities.BitField;

public class PlayerController : MonoBehaviour
{
    enum EInput :byte { LEFT, RIGHT, UP, DOWN };

    Rigidbody2D m_motor;
    Vector2 m_direction;

    private void Awake()
    {
        m_motor = GetComponentInChildren<Rigidbody2D>();
    }

    void Update()
    {
        uint input = 0;

        if (Input.GetKeyDown(KeyCode.A))
            BitField.MarkFlag<EInput>(ref input, EInput.LEFT);
        if (Input.GetKeyDown(KeyCode.D))
            BitField.MarkFlag<EInput>(ref input, EInput.RIGHT);
        if (Input.GetKeyDown(KeyCode.W))
            BitField.MarkFlag<EInput>(ref input, EInput.UP);
        if (Input.GetKeyDown(KeyCode.S))
            BitField.MarkFlag<EInput>(ref input, EInput.DOWN);


        m_direction.x = Input.GetAxis("Horizontal");

        

    }
}

