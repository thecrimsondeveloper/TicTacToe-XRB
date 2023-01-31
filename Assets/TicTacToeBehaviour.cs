using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeBehaviour : MonoBehaviour
{

    protected IEnumerator GrowInTime(GameObject obj, float time, float waitTime = 0)
    {
        float elapsedTime = 0;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        obj.transform.localScale = startScale;
        yield return new WaitForSeconds(waitTime);

        obj.SetActive(true);

        while (elapsedTime < time)
        {
            float ratio = elapsedTime / time;
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, ratio);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        obj.transform.localScale = endScale;
    }
}
