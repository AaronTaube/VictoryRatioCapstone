using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlameFlip : MonoBehaviour
{
	float elapsedTime;
	float timeToFlip;
	Image flameImage;
    // Start is called before the first frame update
    void Start()
    {
		elapsedTime = 0;
		timeToFlip = .075f;
		flameImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
		elapsedTime += Time.deltaTime;
		if(elapsedTime > timeToFlip)
		{
			elapsedTime = 0.0f;
			Flip();
		}
    }
	void Flip()
	{
		flameImage.transform.localScale = new Vector3(-flameImage.transform.localScale.x, 
												flameImage.transform.localScale.y, flameImage.transform.localScale.z);
	}
}
