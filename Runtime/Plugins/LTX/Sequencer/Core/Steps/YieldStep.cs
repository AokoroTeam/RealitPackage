﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Sequencing.Steps
{
    [Serializable]
    public class YieldStep : WaitForFramesStep
    {
        public YieldStep() : base(1)
        {

        }
    }
}
