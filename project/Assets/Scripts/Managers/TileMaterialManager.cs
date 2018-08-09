using UnityEngine;
using System.Collections;

public class TileMaterialManager : MonoBehaviour {
	
	//public instance
	static TileMaterialManager _self = null;
	public static TileMaterialManager TMM { get{ return _self; } }
	
	//materials for the base color and texture of hte tiles
	[SerializeField]
	Material _highTileMaterial;
	
	[SerializeField]
	Material _midTileMaterial;
	
	[SerializeField]
	Material _lowTileMaterial;
	
	//references for the materials to be generated off of
	[SerializeField]
	Material _baserichness;
	
	[SerializeField]
	Material _basehovervalid;	
	
	[SerializeField]
	Material _basetilebase;
	
	//foundation material
	[SerializeField]
	Material _basefoundation;
	
	//array of materials for the tiles
	Material[] _highMaterials = null;
	Material[] _midMaterials = null;
	Material[] _lowMaterials = null;
	
	//richness materials ( 0 - 5 )
	Material[] _richnessMaterials = null;
	
	//hover and valid materials
	[SerializeField]
	Material[] _hoverValidMaterials = null;
	
	//tile base height stripes
	Material[] _tileBaseMaterials = null;
	
	//duplicated foundation material
	Material _foundationMaterial;
	
	
	//color materials for the surface of the tiles
	/*
	Material[] _lowColorMaterials = null;
	Material[] _midColorMaterials = null;
	Material[] _highColorMaterials = null;
	*/
	
	public Material[] RichnessMat 	{ get{ return _richnessMaterials; 	} }
	public Material[] HoverValidMat { get{ return _hoverValidMaterials; } }
	public Material[] TileBaseMat	{ get{ return _tileBaseMaterials;	} }
	
	//returns a material for the base of the tile
	public Material HighMat { get{ return _highMaterials[0]; } }
	public Material MidMat { get{ return _midMaterials[0]; } }
	public Material LowMat { get{ return _lowMaterials[0]; } }
	
	//getter for the foundation mat
	public Material foundationMat { get{ return _foundationMaterial; } }
	
	void Awake()
	{
		_self = this;
				
		//generate the richness materials
		_richnessMaterials = new Material[ 6 ];
		for( int i = 0 ; i < _richnessMaterials.Length ; i ++ )
		{
			_richnessMaterials[i] = Instantiate( _baserichness ) as Material;
			_richnessMaterials[i].mainTextureOffset = new Vector2( (i/6.0f), 0 );
			
			_richnessMaterials[i].color *= 2;
			
			_richnessMaterials[i].renderQueue = 3000 - 6 - i;
		}
		
		//generate the hover valid materials
		_hoverValidMaterials = new Material[ 4 ];
		for( int i = 0 ; i < _hoverValidMaterials.Length ; i ++ )
		{
			_hoverValidMaterials[i] = Instantiate( _basehovervalid ) as Material;
			_hoverValidMaterials[i].mainTextureOffset = new Vector2( (i/4.0f), 0 );
			
			_hoverValidMaterials[i].color *= 1.5f;
			
			_hoverValidMaterials[i].renderQueue = 3000 - i;
		}
		
		//generate the tile base materials
		_tileBaseMaterials = new Material[21];
		for( int i = 0 ; i < _tileBaseMaterials.Length ; i ++ )
		{
			_tileBaseMaterials[i] = Instantiate( _basetilebase ) as Material;
			_tileBaseMaterials[i].mainTextureScale = new Vector2( 2, i * .5f * .25f );
			_tileBaseMaterials[i].mainTextureOffset = new Vector2( 0 , .5f );
			
			_tileBaseMaterials[i].renderQueue = 2000;
		}
		
		//duplicate the materials
		_highMaterials = new Material[1];
		_midMaterials = new Material[1];
		_lowMaterials = new Material[1];
		
		_highMaterials[0] = Instantiate( _highTileMaterial ) as Material;
		_midMaterials[0] = Instantiate( _midTileMaterial ) as Material;
		_lowMaterials[0] = Instantiate( _lowTileMaterial ) as Material;
		
		_highMaterials[0].renderQueue = 2000;
		_midMaterials[0].renderQueue = 2000;
		_lowMaterials[0].renderQueue = 2000;
		
		
		_foundationMaterial = _basefoundation;
		
		
	}
}
