
struct Countdown
{
    public float RemainingTime
    {
        get
        {
            return remainingTime;
        }
    }

    public bool HasExpired()
    {
        return remainingTime <= 0;
    }

    public void Stop()
    {
        Reset(0);
    }

    public void Reset(float seconds)
    {
        remainingTime = seconds;
    }

    public void Elapse(float elapsedSeconds)
    {
        remainingTime -= elapsedSeconds;

        // Clamp to 0
        if (remainingTime < 0)
        {
            remainingTime = 0;
        }
    }

    private float remainingTime;
}