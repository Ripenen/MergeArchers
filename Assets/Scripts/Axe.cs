using UnityEngine;

public class Axe : Arrow
{
    private void LateUpdate()
    {
        if(Collided)
            return;
        
        transform.Rotate(Vector3.right, 360 * Time.deltaTime);
    }
}