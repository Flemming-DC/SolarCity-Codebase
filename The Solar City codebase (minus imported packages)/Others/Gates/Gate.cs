using System.Collections;
using UnityEngine;

public class Gate : APathBlocker
{
    [SerializeField] float openPositionY;
    [SerializeField] float closedPositionY;
    [SerializeField] float openDuration = 3.5f;
    // 0.5f was found to be a sensible value wrt. visuals, but it doesn't match the sound.
    [SerializeField] float closeDuration = 2.1f;
    [SerializeField] bool startAsOpen;
    [SerializeField] Sound openSound;
    [SerializeField] Sound closingSound;

    protected bool open;

    void Awake()
    {
        SetOpenInstantly(startAsOpen);
        openSound?.MakeSource(gameObject);
        closingSound?.MakeSource(gameObject);
    }

    public IEnumerator OpenRoutine()
    {
        if (open)
            Debug.LogWarning("The gate is already opened or partially opened");
        open = true;
        openSound?.Play(gameObject);

        yield return this.EveryFrame(timeSoFar => 
        {
            float nextPositionY = Mathf.Lerp(closedPositionY, openPositionY, timeSoFar / openDuration);
            transform.localPosition = transform.localPosition.With(y: nextPositionY);
        }, openDuration);
    }
    

    public IEnumerator CloseRoutine()
    {
        if (!open)
            Debug.LogWarning("The gate is already closed or partially closed");
        open = false;
        closingSound?.Play(gameObject);

        yield return this.EveryFrame(timeSoFar =>
        {
            float nextPositionY = Mathf.Lerp(openPositionY, closedPositionY, timeSoFar / closeDuration);
            transform.localPosition = transform.localPosition.With(y: nextPositionY);
        }, closeDuration);

    }

    public override void SetPathBlocked(bool block)
    {
        if (open != block)
            return; // the gate is already in the correct state.

        if (block)
            StartCoroutine(CloseRoutine());
        else
            StartCoroutine(OpenRoutine());
    }

    public void SetOpenInstantly(bool open_)
    {
        open = open_;
        transform.localPosition = transform.localPosition.With(y: open ? openPositionY : closedPositionY);
    }

}
