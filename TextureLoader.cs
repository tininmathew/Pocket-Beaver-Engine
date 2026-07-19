using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.IO;

namespace Engine;

public static class TextureLoader
{
    public static int LoadTexture(string path)
    {
        int handle = GL.GenTexture();
        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            
            
            GL.BindTexture(TextureTarget.Texture2D, handle);

            TextureMinFilter filter = TextureMinFilter.Linear;
            if(image.Width <= 128 || image.Height <= 128)
            {
                filter = TextureMinFilter.Nearest;
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);

            StbImage.stbi_set_flip_vertically_on_load(1);

            if(!File.Exists(path)) 
            {
                path = "resources/fallback texture error.bmp";
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            }

            
            
            GL.TexImage2D(TextureTarget.Texture2D, 
                        0, 
                        PixelInternalFormat.Rgba, 
                        image.Width, 
                        image.Height, 
                        0, 
                        PixelFormat.Rgba, 
                        PixelType.UnsignedByte, 
                        image.Data);
        }

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return handle;
    }
}