using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlidingPuzzle : MonoBehaviour
{
    [SerializeField] private RectTransform gameBoard;
    [SerializeField] private Button piecePrefab;

    [SerializeField] private Sprite[] puzzleImages;

    [SerializeField] private int size = 4;
    [SerializeField] private float gap = 5f;

    private List<Button> board = new List<Button>();

    private int emptyLocation;

    private void Start()
    {
        if (puzzleImages == null || puzzleImages.Length != 15)
        {
            Debug.LogError("You must assign exactly 15 puzzle images.");
            return;
        }

        if (gameBoard == null || piecePrefab == null)
        {
            Debug.LogError("Assign Game Board and Piece Prefab.");
            return;
        }

        CreatePuzzle();
    }

    private void CreatePuzzle()
    {
        emptyLocation = size * size - 1;

        float tileWidth = gameBoard.rect.width / size;
        float tileHeight = gameBoard.rect.height / size;

        for (int location = 0; location < size * size - 1; location++)
        {
            Button piece = Instantiate(piecePrefab, gameBoard);

            piece.name = "Piece_" + (location + 1);

            RectTransform pieceRect =
                piece.GetComponent<RectTransform>();

            pieceRect.anchorMin = new Vector2(0.5f, 0.5f);
            pieceRect.anchorMax = new Vector2(0.5f, 0.5f);
            pieceRect.pivot = new Vector2(0.5f, 0.5f);

            pieceRect.sizeDelta = new Vector2(
                tileWidth - gap,
                tileHeight - gap
            );

            pieceRect.anchoredPosition =
                GetPositionFromLocation(location);

            Image image = piece.GetComponent<Image>();

            if (image == null)
            {
                image = piece.GetComponentInChildren<Image>();
            }

            if (image == null)
            {
                Debug.LogError(
                    "The piece prefab needs an Image component."
                );
                return;
            }

            image.sprite = puzzleImages[location];
            image.preserveAspect = true;

            board.Add(piece);

            int clickedLocation = location;

            piece.onClick.AddListener(() =>
            {
                TryMove(clickedLocation);
            });
        }
    }

    private void TryMove(int clickedLocation)
    {
        bool sameRow =
            clickedLocation / size == emptyLocation / size;

        bool sameColumn =
            clickedLocation % size == emptyLocation % size;

        bool nextToHorizontally =
            sameRow &&
            Mathf.Abs(clickedLocation - emptyLocation) == 1;

        bool nextToVertically =
            sameColumn &&
            Mathf.Abs(clickedLocation - emptyLocation) == size;

        if (nextToHorizontally || nextToVertically)
        {
            MovePiece(clickedLocation);
        }
    }

    private void MovePiece(int clickedLocation)
    {
        Button clickedPiece = board[clickedLocation];

        RectTransform pieceRect =
            clickedPiece.GetComponent<RectTransform>();

        pieceRect.anchoredPosition =
            GetPositionFromLocation(emptyLocation);

        board[emptyLocation] = clickedPiece;
        board.RemoveAt(clickedLocation);
        board.Insert(clickedLocation, null);

        emptyLocation = clickedLocation;

        RefreshClickListeners();
    }

    private void RefreshClickListeners()
    {
        for (int location = 0; location < board.Count; location++)
        {
            if (board[location] == null)
                continue;

            Button piece = board[location];

            piece.onClick.RemoveAllListeners();

            int capturedLocation = location;

            piece.onClick.AddListener(() =>
            {
                TryMove(capturedLocation);
            });
        }
    }

    private Vector2 GetPositionFromLocation(int location)
    {
        float tileWidth = gameBoard.rect.width / size;
        float tileHeight = gameBoard.rect.height / size;

        int row = location / size;
        int column = location % size;

        float x =
            -gameBoard.rect.width / 2f
            + tileWidth * column
            + tileWidth / 2f;

        float y =
            gameBoard.rect.height / 2f
            - tileHeight * row
            - tileHeight / 2f;

        return new Vector2(x, y);
    }
}