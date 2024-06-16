using UnityEngine;

public class ParticleHandler : MonoBehaviour
{

    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particleSystem.Play();
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
