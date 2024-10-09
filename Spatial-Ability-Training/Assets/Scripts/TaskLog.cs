using System;


public class TaskLog
{
    string cube;
    int angle;
    float time;

    public TaskLog(string cube, int angle, float time)
    {
        this.cube = cube;
        this.angle = angle;
        this.time = time;

    }

    public override string ToString()
    {
        return $"{cube}, {angle}, {time.ToString().Replace(",", ".")}";
    }

}
