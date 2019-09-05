using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AudioInterface  {

    void PlaySound(string name, string path,Vector3 pos ,object userData);
}
