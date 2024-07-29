using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class TreeItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
  {
    [SerializeField] private TextMeshProUGUI _text;
    [Header("Spawn Param's")]
    [SerializeField] private GameObject _model;
    [SerializeField] private float _minModelSize = 0.95f;
    [SerializeField] private float _maxModelSize = 1.05f;
    [Header("Effect")]
    [SerializeField] private GameObject _correctParticles;
    [SerializeField] private GameObject _incorrectParticles;
    [Header("Sound")]
    [SerializeField] private AudioClip _soundEffect;

    public string Answer { get; private set; }
    public AudioClip NarrationClip { get; private set; }

    private Vector3 _initLocalPosition;
    private Vector3 _offset;
    private bool _placed = false;

    private Camera _mainCamera;
    private AudioSource _audioSource;

    private void Awake()
    {
      _mainCamera = Camera.main;
      _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
      _initLocalPosition = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
      if (other.TryGetComponent(out Basket basket))
      {
        basket.PlaceItem(this);

        _placed = true;
      }
    }

    public void SetRandomModelSize()
    {
      float randomSize = Random.Range(_minModelSize, _maxModelSize);

      _model.transform.localScale = new Vector3(randomSize, randomSize);
    }

    public void SetAnswer(string answer)
    {
      Answer = answer;
      _text.text = Answer;
    }

    public void SetNarrationClip(AudioClip audioClip)
    {
      NarrationClip = audioClip;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      _offset = transform.position - _mainCamera.ScreenToWorldPoint(
        new Vector3(eventData.position.x, eventData.position.y, transform.position.z));

      PlaySoundEffect();

      EventBus<EventStructs.TreeItemClickEvent>.Raise(new EventStructs.TreeItemClickEvent { ItemAudioClip = NarrationClip });
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (_placed == true) return;

      Vector3 newPosition = new Vector3(eventData.position.x, eventData.position.y, transform.position.z);

      transform.position = _mainCamera.ScreenToWorldPoint(newPosition) + _offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      if (_placed == true) return;

      transform.DOLocalMove(_initLocalPosition, 0.25f);
    }

    public void SpawnParticles(bool correct)
    {
      if (correct == true)
        Instantiate(_correctParticles, transform.position, Quaternion.identity);
      else
        Instantiate(_incorrectParticles, transform.position, Quaternion.identity);
    }

    private void PlaySoundEffect()
    {
      float randomPitch = Random.Range(0.95f, 1.05f);

      _audioSource.pitch = randomPitch;
      _audioSource.PlayOneShot(_soundEffect);
    }
  }
}