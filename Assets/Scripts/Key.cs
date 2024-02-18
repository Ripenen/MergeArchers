using DG.Tweening;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void Start()
    {
        transform.DOMoveY(transform.position.y + 0.5f, 1).OnComplete(() =>
        {
            transform.DOMoveY(transform.position.y - 0.5f, 1);
        }).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
    }
}