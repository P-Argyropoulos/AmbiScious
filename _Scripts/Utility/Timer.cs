using System;


namespace Utility {

    public abstract class Timer 
    {
        protected float initialTime;
        protected float currentTime { get; set; }

        public bool isRunning {get; protected set;}

        public Action OnTimerStart = delegate {};
        public Action OnTimerStop = delegate {};

        protected Timer (float value)
        {
            initialTime = value;
            isRunning  = false;
        }

        public float Progress => currentTime / initialTime;

        public void Start()
        {
            currentTime = initialTime;
            if (!isRunning)
            {
                isRunning = true;
                OnTimerStart.Invoke();
                
            } 
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public abstract void Tick(float deltaTime);
    }


    public class CooldownTimer :Timer
    {
        public  CooldownTimer (float value) : base(value) {}

        public override void Tick (float deltaTime)
        {
            if (isRunning && currentTime > 0)
            {
                currentTime -= deltaTime;
            }

            if (isRunning && currentTime <= 0)
            {
                Stop();
            }
        }
        // Extra functionalities of Cooldown timer for future implementations
        public bool isFinished => currentTime < 0;

        public void Reset() => currentTime = initialTime;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }


    // class for future implementation
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer () : base (0) {}

        public override void Tick (float deltaTime)
        {
            if (isRunning)  
            {  
                currentTime += deltaTime;
            }
        }

        public void Reset() => currentTime = 0;

        public float GetTime() => currentTime;

    }
}