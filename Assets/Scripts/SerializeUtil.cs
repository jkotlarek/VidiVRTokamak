
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// NOTE: This serializer is not faster than reader ASCII data!
/// </summary>
public static class Serializer
{
    public static void Serialize(List<Particle> list, string filename)
    {
        BinaryFormatter bf = new BinaryFormatter();
        
        using (var stream = File.Open(filename, FileMode.Create))
        {
            SurrogateSelector ss = new SurrogateSelector();
            Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);
            bf.SurrogateSelector = ss;
            
            bf.Serialize(stream, list);
        }

    }

    public static List<Particle> Deserialize(string filename)
    {
        BinaryFormatter bf = new BinaryFormatter();

        using (var stream = File.Open(filename, FileMode.Open))
        {
            SurrogateSelector ss = new SurrogateSelector();
            Vector3SerializationSurrogate v3ss = new Vector3SerializationSurrogate();
            ss.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3ss);
            bf.SurrogateSelector = ss;
            
            return (List<Particle>)bf.Deserialize(stream);
        }
    }
}

[Serializable]
public class Particle
{
    public List<Vector3> timesteps;
    public List<bool> trapped;

    public Particle()
    {
        timesteps = new List<Vector3>();
        trapped = new List<bool>();
    }
    public Particle(List<bool> b)
    {
        timesteps = new List<Vector3>();
        trapped = b;
    }
}


sealed class Vector3SerializationSurrogate : ISerializationSurrogate
{

    // Method called to serialize a Vector3 object
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {

        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    // Method called to deserialize a Vector3 object
    public System.Object SetObjectData(System.Object obj,
                                       SerializationInfo info, StreamingContext context,
                                       ISurrogateSelector selector)
    {

        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;
        return obj;   // Formatters ignore this return value //Seems to have been fixed!
    }
}