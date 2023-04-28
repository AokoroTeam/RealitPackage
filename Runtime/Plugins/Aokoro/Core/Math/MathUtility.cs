﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro
{
    public class MathUtility
    {
        public static float TriangleArea(Vector3 p1, Vector3 p2, Vector3 p3) => Vector3.Cross(p1 - p2, p1 - p3).magnitude * 0.5f;
        public static bool LineIntersection(out Vector2 intersection, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            intersection = Vector3.zero;

            float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
            float x1lo, x1hi, y1lo, y1hi;

            Ax = p2.x - p1.x;
            Bx = p3.x - p4.x;

            // X bound box test/
            if (Ax < 0)
            {
                x1lo = p2.x; 
                x1hi = p1.x;
            }
            else
            {
                x1hi = p2.x; 
                x1lo = p1.x;
            }

            if (Bx > 0)
            {
                if (x1hi < p4.x || p3.x < x1lo) 
                    return false;
            }
            else if (x1hi < p3.x || p4.x < x1lo)
                return false;

            Ay = p2.y - p1.y;
            By = p3.y - p4.y;

            // Y bound box test//

            if (Ay < 0)
            {
                y1lo = p2.y; 
                y1hi = p1.y;
            }
            else
            {
                y1hi = p2.y; 
                y1lo = p1.y;
            }



            if (By > 0)
            {
                if (y1hi < p4.y || p3.y < y1lo) return false;
            }
            else if (y1hi < p3.y || p4.y < y1lo) return false;

            Cx = p1.x - p3.x;
            Cy = p1.y - p3.y;

            d = By * Cx - Bx * Cy;  // alpha numerator//
            f = Ay * Bx - Ax * By;  // both denominator//

            // alpha tests//

            if (f > 0)
            {
                if (d < 0 || d > f) return false;
            }
            else
            {
                if (d > 0 || d < f) return false;
            }

            e = Ax * Cy - Ay * Cx;  // beta numerator//

            // beta tests //

            if (f > 0)
            {
                if (e < 0 || e > f) return false;
            }
            else if (e > 0 || e < f) return false;

            // check if they are parallel

            if (f == 0) return false;

            // compute intersection coordinates //

            num = d * Ax; // numerator //

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //

            //    intersection.x = p1.x + (num+offset) / f;
            intersection.x = p1.x + num / f;
            num = d * Ay;

            //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
            //    intersection.y = p1.y + (num+offset) / f;
            intersection.y = p1.y + num / f;

            return true;
        }
    }
}
