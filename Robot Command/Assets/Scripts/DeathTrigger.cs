using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip[] _deathSounds;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            _audioSource.PlayOneShot(_deathSounds[Random.Range(0, _deathSounds.Length)]);

            player.GetComponent<Animator>().SetBool("IsMoving", false);
        }
    }
}
