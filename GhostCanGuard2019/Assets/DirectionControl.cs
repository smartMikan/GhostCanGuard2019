﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;       


    private Quaternion m_RelativeRotation;         


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.parent.rotation = m_RelativeRotation;
    }
    
}
