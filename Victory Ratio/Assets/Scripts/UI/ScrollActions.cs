using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrollActions : MonoBehaviour
{
	[SerializeField]
	private Tilemap mainTilemap;
	[SerializeField]
	private Camera camera;
	private Transform cameraTransform;
	[SerializeField]
	private float cameraSpeed;
	[Header("Scroll Zones")]
	[SerializeField]
	GameObject leftArrow;
	[SerializeField]
	GameObject topArrow, rightArrow, bottomArrow, topLeftArrow, topRightArrow, bottomLeftArrow, bottomRightArrow;
	[SerializeField]
	private float maxX, maxY, minX, minY;

	private bool moveLeft = false, moveUp = false, moveRight = false, moveDown = false;
	// Start is called before the first frame update
	void Start()
    {
		topArrow.SetActive(false);
		rightArrow.SetActive(false);
		bottomArrow.SetActive(false);
		leftArrow.SetActive(false);
		topLeftArrow.SetActive(false);
		topRightArrow.SetActive(false);
		bottomLeftArrow.SetActive(false);
		bottomRightArrow.SetActive(false);
		cameraTransform = camera.transform;
	}

    // Update is called once per frame
    void Update()
    {
		if (moveLeft)
		{
			cameraTransform.Translate(Vector3.left * Time.deltaTime * cameraSpeed);
			
		}
		if (moveRight)
		{
			cameraTransform.Translate(Vector3.right * Time.deltaTime * cameraSpeed);
			
		}
		
		if (moveUp)
		{
			cameraTransform.Translate(Vector3.up * Time.deltaTime * cameraSpeed);
			
		}
		if (moveDown)
		{
			cameraTransform.Translate(Vector3.down * Time.deltaTime * cameraSpeed);
			
		}
		//No matter what moves the camera, with these values out here it should not go outside of the camera range.
		if (cameraTransform.position.x < minX)
		{
			cameraTransform.position = new Vector3(minX, cameraTransform.position.y, cameraTransform.position.z);
		}
		if (cameraTransform.position.x > maxX)
		{
			cameraTransform.position = new Vector3(maxX, cameraTransform.position.y, cameraTransform.position.z);
		}
		if (cameraTransform.position.y > maxY)
		{
			cameraTransform.position = new Vector3(cameraTransform.position.x, maxY, cameraTransform.position.z);
		}
		if (cameraTransform.position.y < minY)
		{
			cameraTransform.position = new Vector3(cameraTransform.position.x, minY, cameraTransform.position.z);
		}
	}

	public void MoveLeft()
	{
		moveLeft = true;
		leftArrow.SetActive(true);
	}
	public void StopMoveLeft()
	{
		moveLeft = false;
		leftArrow.SetActive(false);
	}
	public void MoveRight()
	{
		moveRight = true;
		rightArrow.SetActive(true);
	}
	public void StopMoveRight()
	{
		moveRight = false;
		rightArrow.SetActive(false);
	}
	public void MoveUp()
	{
		moveUp = true;
		topArrow.SetActive(true);
	}
	public void StopMoveUp()
	{
		moveUp = false;
		topArrow.SetActive(false);
	}
	public void MoveDown()
	{
		moveDown = true;
		bottomArrow.SetActive(true);
	}
	public void StopMoveDown()
	{
		moveDown = false;
		bottomArrow.SetActive(false);
	}
	public void MoveUpLeft()
	{
		moveUp = true;
		moveLeft = true;
		topLeftArrow.SetActive(true);
	}
	public void StopMoveUpLeft()
	{
		moveUp = false;
		moveLeft = false;
		topLeftArrow.SetActive(false);
	}
	public void MoveUpRight()
	{
		moveUp = true;
		moveRight = true;
		topRightArrow.SetActive(true);
	}
	public void StopMoveUpRight()
	{
		moveUp = false;
		moveRight = false;
		topRightArrow.SetActive(false);
	}
	public void MoveDownLeft()
	{
		moveDown = true;
		moveLeft = true;
		bottomLeftArrow.SetActive(true);
	}
	public void StopMoveDownLeft()
	{
		moveDown = false;
		moveLeft = false;
		bottomLeftArrow.SetActive(false);
	}
	public void MoveDownRight()
	{
		moveDown = true;
		moveRight = true;
		bottomRightArrow.SetActive(true);
	}
	public void StopMoveDownRight()
	{
		moveDown = false;
		moveRight = false;
		bottomRightArrow.SetActive(false);
	}
}
