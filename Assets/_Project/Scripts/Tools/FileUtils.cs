using UnityEngine;
using UnityEngine.Assertions;

using FullSerializer;

namespace TestProject
{
    public static class FileUtils
    {
        /// <summary>
        /// Loads the specified json file
        /// </summary>
        public static T LoadJsonFile<T>(fsSerializer serializer, string path) where T : class
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            Assert.IsNotNull((textAsset));
            fsData data = fsJsonParser.Parse(textAsset.text);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();
            return deserialized as T;
        }
    }
}
