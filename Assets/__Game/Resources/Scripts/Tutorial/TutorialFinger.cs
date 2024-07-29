using DG.Tweening;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Tutorial
{
  public class TutorialFinger : MonoBehaviour
  {
    [SerializeField] private GameObject _model;

    public void EnableFinger()
    {
      _model.transform.localScale = Vector3.zero;

      gameObject.SetActive(true);
      _model.transform.DOScale(1, 0.2f);
    }

    public void SetMovement(Transform targetFrom, Transform targetTo)
    {
      transform.position = targetFrom.position;

      transform.DOMove(targetTo.position, 1.5f)
        .SetLoops(-1)
        .SetEase(Ease.InOutQuad);
    }

    public void StopAndDisableFinger()
    {
      _model.transform.DOScale(0, 0.2f)
        .OnComplete(() =>
      {
        DOTween.Kill(transform);
        gameObject.SetActive(false);
      });
    }
  }
}