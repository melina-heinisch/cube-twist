using System;


public class TaskLog
{
    private int id;
    string cube;
    int angle;
    float time;

    public TaskLog(int id, string cube, int angle, float time)
    {
        this.id = id;
        this.cube = cube;
        this.angle = angle;
        this.time = time;

    }

    public override string ToString()
    {
        return $"{id},{cube}, {angle}, {time.ToString().Replace(",", ".")}";
    }

}
