using UnityEngine;
using System.Collections;

public class CyalumeController : MonoBehaviour
{
	private ParticleSystem particleSystem_;

	public Color baseColor {
		get { return GetComponent<Renderer>().material.GetColor("_BaseColor"); }
		set { GetComponent<Renderer>().material.SetColor("_BaseColor", value); }
	}

	public float waveX {
		get { return GetComponent<Renderer>().material.GetFloat("_WaveFactorX"); }
		set { GetComponent<Renderer>().material.SetFloat("_WaveFactorX", value); }
	}

	public float waveZ {
		get { return GetComponent<Renderer>().material.GetFloat("_WaveFactorZ"); }
		set { GetComponent<Renderer>().material.SetFloat("_WaveFactorZ", value); }
	}

	public float waveCorrection {
		get { return GetComponent<Renderer>().material.GetFloat("_WaveCorrection"); }
		set { GetComponent<Renderer>().material.SetFloat("_WaveCorrection", value); }
	}

	public float delayByDistance {
		get { return GetComponent<Renderer>().material.GetFloat("_Delay"); }
		set { GetComponent<Renderer>().material.SetFloat("_Delay", value); }
	}

	public float wavePitch {
		get { return GetComponent<Renderer>().material.GetFloat("_Pitch"); }
		set { GetComponent<Renderer>().material.SetFloat("_Pitch", value); }
	}

	public float bend {
		get { return GetComponent<Renderer>().material.GetFloat("_Bend"); }
		set { GetComponent<Renderer>().material.SetFloat("_Bend", value); }
	}

	public Texture texture {
		get { return GetComponent<Renderer>().material.GetTexture("_MainTex"); }
		set { GetComponent<Renderer>().material.SetTexture("_MainTex", value); }
	}

	public Color startColor {
		get { return particleSystem_.startColor; }
		set { particleSystem_.startColor = value; }
	}

    private Vector3 startPosition;

	void Start()
	{
		particleSystem_ = GetComponent<ParticleSystem>();
        startPosition = transform.position;
	}

    void Update()
    {
        Vector3 pos = startPosition + new Vector3(0,0.7f,0);
        float t = (Time.time % 1)*5;
        transform.position = Vector3.Lerp(startPosition,pos, Mathf.Log((1 - Mathf.Cos(t) / (1 + Mathf.Cos(t)))));
    }
}
