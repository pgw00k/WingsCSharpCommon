using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenOcean.Common;

namespace Wings
{
    public class HackGUI :MonoBehaviour
    {
        protected virtual void Start()
        {
            UnityHack.Program.Load();
        }
    }
}
