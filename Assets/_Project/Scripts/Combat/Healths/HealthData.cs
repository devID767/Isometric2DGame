namespace Isometric2DGame.Combat.Healths
{
    public readonly struct HealthData
    {
        public int Max { get; }
        public int Current { get; }

        public bool IsAlive => Current > 0;

        public float Normalized => Max > 0 ? (float)Current / Max : 0f;

        public HealthData(int current, int max)
        {
            Current = current;
            Max = max;
        }
    }
}
