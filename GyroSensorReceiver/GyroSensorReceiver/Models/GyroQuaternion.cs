﻿using System;

public class GyroQuaternion
{
    public string type = "gyroData";
    public string sensorName { get; set; }
    public float qX { get; set; }
    public float qY { get; set; }
    public float qZ { get; set; }
    public float qW { get; set; }

    public GyroQuaternion(string SensorName, float X, float Y, float Z, float W)
    {
        sensorName = SensorName;
        qX = X;
        qY = Y;
        qZ = Z;
        qW = W;
    }

    public static GyroQuaternion Inverse(GyroQuaternion quat)
    {
        GyroQuaternion quaternion2 = new GyroQuaternion(quat.sensorName, 0, 0, 0, -1);
        float num2 = quat.qX * quat.qX + quat.qY * quat.qY + quat.qZ * quat.qZ + quat.qW * quat.qW;
        float num = 1f / num2;

        quaternion2.qX = -quat.qX * num;
        quaternion2.qY = -quat.qY * num;
        quaternion2.qZ = -quat.qZ * num;
        quaternion2.qW = quat.qW * num;
        return quaternion2;
    }

    public static GyroQuaternion Multiply(GyroQuaternion q1, GyroQuaternion q2)
    {
        float qX = q1.qW * q2.qX + q1.qX * q2.qW + q1.qY * q2.qZ - q1.qZ * q2.qY;
        float qY = q1.qW * q2.qY + q1.qY * q2.qW + q1.qZ * q2.qX - q1.qX * q2.qZ;
        float qZ = q1.qW * q2.qZ + q1.qZ * q2.qW + q1.qX * q2.qY - q1.qY * q2.qX;
        float qW = q1.qW * q2.qW - q1.qX * q2.qX - q1.qY * q2.qY - q1.qZ * q2.qZ;
        return new GyroQuaternion(q1.sensorName, qX, qY, qZ, qW);
    }

    public static Point3D QuatToEuler(GyroQuaternion quat)
    {
        Point3D point3D = new Point3D();

        float X = quat.qX;
        float Y = quat.qY;
        float Z = quat.qZ;
        float W = quat.qW;

        float singularityTest = Z * X - W * Y;
        float yawY = 2f * (W * Z + X * Y);
        float yawX = 1f - 2f * (Y * Y + Z * Z);

        const float RAD_TO_DEG = (float)(180 / Math.PI);

        if (singularityTest < -0.4999995f)
        {
            point3D.X = -90f;
            point3D.Y = (float)Math.Atan2(yawY, yawX) * RAD_TO_DEG;
            point3D.Z = NormalizeAxis((float)(-point3D.Y - 2f * Math.Atan2(X, W) * RAD_TO_DEG));
        }
        else if (singularityTest > 0.4999995f)
        {
            point3D.X = 90f;
            point3D.Y = (float)Math.Atan2(yawY, yawX) * RAD_TO_DEG;
            point3D.Z = NormalizeAxis((float)(point3D.Y - 2f * Math.Atan2(X, W) * RAD_TO_DEG));
        }
        else
        {
            point3D.X = (float)Math.Asin(2f * singularityTest) * RAD_TO_DEG;
            point3D.Y = (float)Math.Atan2(yawY, yawX) * RAD_TO_DEG;
            point3D.Z = (float)Math.Atan2(-2f * (W * X + Y * Z), 1f - 2f * (X * X + Y * Y)) * RAD_TO_DEG;
        }

        return point3D;
    }

    private static float NormalizeAxis(float angle)
    {
        angle = FMod(angle, 360);
        if (angle < 0f)
            angle += 360f;
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }

    private static float FMod(float x, float y)
    {
        if (Math.Abs(y) <= 1E-8f)
            return 0;

        float quotient = (int)(x / y);
        float intPortion = y * quotient;

        if (Math.Abs(intPortion) > Math.Abs(x))
            intPortion = x;

        float result = x - intPortion;
        return result;
    }
}

