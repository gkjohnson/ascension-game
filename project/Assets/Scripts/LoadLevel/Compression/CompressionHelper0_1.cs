using UnityEngine;
using System.Collections;
using System.IO;
using System;

//Compresses and Decompresses byte arrays using the LZMA compression algorithm
abstract public class CompressionHelper0_1 : MonoBehaviour {
	
	//compresses the given byte array
	public static byte[] Compress( byte[] uncbytes )
	{
		if( uncbytes == null ) return null;
		
		//create the encoder object
		SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
		
		//create the memory streams
		MemoryStream input = new MemoryStream(uncbytes);
		MemoryStream output = new MemoryStream();
		
		//set the properties of the coder
		coder.WriteCoderProperties( output );
		
		//write the size of the array
		output.Write(System.BitConverter.GetBytes( input.Length ), 0, 8);
		
		//encode the byte array
		coder.Code( input, output, input.Length, -1, null );
		
		//return the compressed array
		return output.GetBuffer();
		
	}
	
	//decompresses the given byte array
	public static byte[] Decompress( byte[] combytes )
	{
		if( combytes == null ) return null;
		
		//create the decoder object
		SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
		
		//create the memory streams;
		MemoryStream input = new MemoryStream( combytes );
		MemoryStream output = new MemoryStream();
		
		//set the properties of teh decoder
	    byte[] properties = new byte[5];
        input.Read(properties, 0, 5);
		coder.SetDecoderProperties( properties );
		
		//read the length of the original file
		byte[] filelendata = new byte[8];
		input.Read(filelendata, 0, 8);
		long filelen = System.BitConverter.ToInt64( filelendata, 0 );
		
		//decode the byte array
		coder.Code( input, output, input.Length, filelen, null );
		
		//return the decompressed array
		return output.GetBuffer();
	}
}
