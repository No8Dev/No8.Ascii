namespace Asciis.App.ParticleSystem;

public class Particle
{
    public VecF Location { get; set; }
    public VecF? Velocity { get; set; }
    public VecF? Acelleration { get; set; }
    public float? Lifespan { get; private set; }
    public float? RemainingLifespan { get; private set; }

    public delegate void DrawDelegate(Particle particle, Canvas canvas);
    public DrawDelegate? OnDraw { get; set; }

    public float PercentRemaining => RemainingLifespan.HasValue ? RemainingLifespan.Value / Lifespan!.Value : 1f;
    public bool Completed => RemainingLifespan.HasValue && RemainingLifespan.Value < 0f;

    public Particle(
        VecF location, 
        VecF? velocity = null, 
        VecF? acelleration = null, 
        float? lifespan = null,
        DrawDelegate? onDraw = null)
    {
        Location = location;
        Velocity = velocity;
        Acelleration = acelleration;
        RemainingLifespan = Lifespan = lifespan;
        OnDraw = onDraw;
    }

    public void Update(float elapsed)
    {
        RemainingLifespan -= elapsed;
        if (Acelleration is not null && Velocity is not null)
            Velocity += (Acelleration * elapsed);
        if (Velocity is not null)
            Location += (Velocity * elapsed);
    }

    public virtual void Draw(Canvas canvas) => OnDraw?.Invoke(this, canvas);
}


public class ParticleSystem
{
    private readonly List<Particle> _particles = new();

    public void Update(float elapsed)
    {
        foreach (var particle in _particles.ToArray())
        {
            particle.Update(elapsed);
            if (particle.Completed)
                _particles.Remove(particle);
        }
    }

    public void Draw(Canvas canvas)
    {
        foreach (var particle in _particles.ToArray())
            particle.Draw(canvas);
    }

    public void Add(Particle particle) => _particles.Add(particle);
    public void Remove(Particle particle) => _particles.Remove(particle);
    public void Clear() => _particles.Clear();
}
