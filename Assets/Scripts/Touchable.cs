using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Touchable
{
    void Touched(Vector3 at);

    void Moved(Vector3 to);

    void Released(Vector3 at);
}
