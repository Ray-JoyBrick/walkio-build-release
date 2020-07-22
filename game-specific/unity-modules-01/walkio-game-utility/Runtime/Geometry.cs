namespace JoyBrick.Walkio.Game.Utility
{
    using UnityEngine;

    public static class Geometry
    {
        public static Mesh CreatePlane(int width, int height)
        {
        
            var mesh = new Mesh();
            mesh.name = "Procedural Grid";
        
            //calculate number of vertices
            var vertices = new Vector3[(width + 1) * (height + 1)];
        
            Vector2[] uv = new Vector2[vertices.Length];
        
            //positioning vertices and providing Uv coordinates
            for (int i = 0, y = 0; y <= height; y++)
            {
                for (int x = 0; x <= width; x++, i++)
                {
                    vertices[i] = new Vector3(x, y);
                    uv[i] = new Vector2((float)x / width,(float) y / height);
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
        
            int[] triangles = new int[width * height * 6];
            for (int ti = 0, vi = 0, y = 0; y < height; y++, vi++)
            {
                for (int x = 0; x < width; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + width + 1;
                    triangles[ti + 5] = vi + width + 2;
                }
            }
            mesh.triangles = triangles;
        
            mesh.RecalculateNormals();
        
            return mesh;
        }
    }
}