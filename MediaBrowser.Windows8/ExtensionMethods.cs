﻿using MediaBrowser.Model.Dto;
using MediaBrowser.Windows8.Model;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace MediaBrowser.Windows8
{
    public static class ExtensionMethods
    {
        public static byte[] ToHash(this string s)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var buff = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var array = new byte[0];
            CryptographicBuffer.CopyToByteArray(hashed, out array);
            return array;
        }

        public static GridItemWrapper<BaseItemDto> ToWrapper(this BaseItemDto item, int rowSpan = 1, int colSpan = 1)
        {
            return new GridItemWrapper<BaseItemDto>(item){ColSpan = colSpan, RowSpan = rowSpan};
        }
    }
}