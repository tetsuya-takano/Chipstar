﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {

    [SerializeField] private Object[] m_list = null;

    public Object[] List { get { return m_list; } }
}
