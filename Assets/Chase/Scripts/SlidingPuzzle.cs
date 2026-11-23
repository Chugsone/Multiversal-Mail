using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SlidingPuzzle : MonoBehaviour
{
    [SerializeField] private Canvas puzzleCanvas;
    [SerializeField] private RectTransform gameBoard;
    [SerializeField] private Button piecePrefab;
    [SerializeField] private Sprite[] puzzleImages;
    [SerializeField] private int size = 4;
    [SerializeField] private int shuffleMoves = 100;

    private List<Button> board = new List<Button>();
    private List<Button> correctOrder = new List<Button>();
    private int emptyLocation;
    private int selectedLocation = -1;
    private bool isShuffling = false;
    private bool isSolved = false;

    private void Start()
    {
        if (puzzleImages == null || puzzleImages.Length != (size * size) - 1)
            return;

        if (gameBoard == null || piecePrefab == null)
            return;

        if (puzzleCanvas == null)
        {
            puzzleCanvas = gameBoard.GetComponentInParent<Canvas>();
        }

        LayoutGroup layoutGroup = gameBoard.GetComponent<LayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameBoard);

        CreatePuzzle();
        ShufflePuzzle();
    }

    private void Update()
    {
        if (isSolved || selectedLocation == -1) return;

        int targetLocation = -1;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedLocation / size > 0)
                targetLocation = selectedLocation - size;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedLocation / size < size - 1)
                targetLocation = selectedLocation + size;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedLocation % size > 0)
                targetLocation = selectedLocation - 1;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedLocation % size < size - 1)
                targetLocation = selectedLocation + 1;
        }

        if (targetLocation >= 0 && targetLocation < size * size && targetLocation == emptyLocation)
        {
            MovePiece(selectedLocation);
        }
    }

    private void CreatePuzzle()
    {
        board.Clear();
        correctOrder.Clear();
        emptyLocation = size * size - 1;

        for (int location = 0; location < size * size - 1; location++)
        {
            Button piece = Instantiate(piecePrefab, gameBoard, false);
            piece.name = "Piece_" + (location + 1);

            RectTransform pieceRect = piece.GetComponent<RectTransform>();
            pieceRect.anchorMin = new Vector2(0.5f, 0.5f);
            pieceRect.anchorMax = new Vector2(0.5f, 0.5f);
            pieceRect.pivot = new Vector2(0.5f, 0.5f);
            pieceRect.localScale = Vector3.one;
            pieceRect.anchoredPosition = GetPositionFromLocation(location);

            Image image = piece.GetComponent<Image>();
            if (image == null)
            {
                image = piece.GetComponentInChildren<Image>();
            }

            if (image == null)
                return;

            image.sprite = puzzleImages[location];

            board.Add(piece);
            correctOrder.Add(piece);

            int clickedLocation = location;
            piece.onClick.AddListener(() => OnPieceClicked(clickedLocation));
        }

        board.Add(null);
        correctOrder.Add(null);
    }

    private void ShufflePuzzle()
    {
        isShuffling = true;

        for (int i = 0; i < shuffleMoves; i++)
        {
            List<int> validNeighbors = new List<int>();

            int emptyRow = emptyLocation / size;
            int emptyCol = emptyLocation % size;

            if (emptyRow > 0) validNeighbors.Add(emptyLocation - size);
            if (emptyRow < size - 1) validNeighbors.Add(emptyLocation + size);
            if (emptyCol > 0) validNeighbors.Add(emptyLocation - 1);
            if (emptyCol < size - 1) validNeighbors.Add(emptyLocation + 1);

            int randomNeighbor = validNeighbors[Random.Range(0, validNeighbors.Count)];
            MovePiece(randomNeighbor);
        }

        isShuffling = false;
        ClearSelection();

        if (CheckIfSolved())
        {
            ShufflePuzzle();
        }
    }

    private void OnPieceClicked(int clickedLocation)
    {
        if (isSolved) return;

        SelectPiece(clickedLocation);

        if (IsAdjacentToEmpty(clickedLocation))
        {
            MovePiece(clickedLocation);
        }
    }

    private void SelectPiece(int location)
    {
        ClearSelection();
        selectedLocation = location;

        if (selectedLocation != -1 && board[selectedLocation] != null)
        {
            board[selectedLocation].GetComponent<Image>().color = new Color(1f, 1f, 0.7f, 1f);
        }
    }

    private void ClearSelection()
    {
        if (selectedLocation != -1 && board[selectedLocation] != null)
        {
            board[selectedLocation].GetComponent<Image>().color = Color.white;
        }
        selectedLocation = -1;
    }

    private bool IsAdjacentToEmpty(int location)
    {
        bool sameRow = location / size == emptyLocation / size;
        bool sameColumn = location % size == emptyLocation % size;
        bool nextToHorizontally = sameRow && Mathf.Abs(location - emptyLocation) == 1;
        bool nextToVertically = sameColumn && Mathf.Abs(location - emptyLocation) == size;

        return nextToHorizontally || nextToVertically;
    }

    private void MovePiece(int fromLocation)
    {
        Button pieceToMove = board[fromLocation];
        RectTransform pieceRect = pieceToMove.GetComponent<RectTransform>();
        pieceRect.anchoredPosition = GetPositionFromLocation(emptyLocation);

        int newLocation = emptyLocation;

        board[emptyLocation] = pieceToMove;
        board[fromLocation] = null;
        emptyLocation = fromLocation;

        if (selectedLocation == fromLocation)
        {
            selectedLocation = newLocation;
        }

        RefreshClickListeners();

        if (!isShuffling && CheckIfSolved())
        {
            OnPuzzleCompleted();
        }
    }

    private bool CheckIfSolved()
    {
        for (int i = 0; i < board.Count; i++)
        {
            if (board[i] != correctOrder[i])
            {
                return false;
            }
        }
        return true;
    }

    private void OnPuzzleCompleted()
    {
        isSolved = true;
        ClearSelection();

        if (puzzleCanvas != null)
        {
            puzzleCanvas.gameObject.SetActive(false);
        }
    }

    private void RefreshClickListeners()
    {
        for (int location = 0; location < board.Count; location++)
        {
            if (board[location] == null) continue;

            Button piece = board[location];
            piece.onClick.RemoveAllListeners();
            int capturedLocation = location;
            piece.onClick.AddListener(() => OnPieceClicked(capturedLocation));
        }
    }

    private Vector2 GetPositionFromLocation(int location)
    {
        float width = gameBoard.rect.width;
        float height = gameBoard.rect.height;

        float tileWidth = width / size;
        float tileHeight = height / size;

        int row = location / size;
        int column = location % size;

        float x = -width / 2f + tileWidth * column + tileWidth / 2f;
        float y = height / 2f - tileHeight * row - tileHeight / 2f;

        float maxX = (width - tileWidth) / 2f;
        float maxY = (height - tileHeight) / 2f;
        x = Mathf.Clamp(x, -maxX, maxX);
        y = Mathf.Clamp(y, -maxY, maxY);

        return new Vector2(x, y);
    }
}
