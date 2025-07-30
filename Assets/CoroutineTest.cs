using System.Collections;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{
    private int counter = 0;
    private bool go = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (go == true)
        {
            go = false;
            StartCoroutine(CoroutineTestFoo(0));
        }
    }

    public IEnumerator CoroutineTestFoo(int depth)
    {
        if (depth > 50)
        {
            Debug.Log(counter);
            counter++;
            go = true;
        }
        else
        {
            // yield return null;
            yield return CoroutineTestFoo(depth + 1);
        }
        
        yield break;
    }
}
