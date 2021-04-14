using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	[SerializeField]
	AudioClip[] clips;
	AudioSource source;
	int index;
    // Start is called before the first frame update
    void Start()
    {
		index = 0;
		source = GetComponent<AudioSource>();
		source.clip = clips[0];
		source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void PlayNext()
	{
		if(index + 1 < clips.Length)
		{
			index++;
			source.clip = clips[index];
			source.Play();
		}
	}
}
