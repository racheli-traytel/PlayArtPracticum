import React, { useRef, useState, useEffect } from 'react';
import { Button, Slider, Grid, Typography, Box } from '@mui/material';
import { Upload, Print, Palette, Brush } from '@mui/icons-material';

const PaintCanvas = () => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const ctxRef = useRef<CanvasRenderingContext2D | null>(null);
  const [color, setColor] = useState<string>('#000000');
  const [brushSize, setBrushSize] = useState<number>(5);
  const [isDrawing, setIsDrawing] = useState<boolean>(false);
  const [isEraseMode, setIsEraseMode] = useState<boolean>(false);
  const [image, setImage] = useState<string | null>(null);

  useEffect(() => {
    if (canvasRef.current) {
      const canvas = canvasRef.current;
      canvas.width = 800;
      canvas.height = 600;
      ctxRef.current = canvas.getContext('2d');
      if (image) {
        const img = new Image();
        img.src = image;
        img.onload = () => {
          if (ctxRef.current && canvasRef.current) {
            ctxRef.current.drawImage(img, 0, 0, canvasRef.current.width, canvasRef.current.height);
          }
        };
      }
    }
  }, [image]);

  const handleMouseDown = (e: React.MouseEvent<HTMLCanvasElement>) => {
    console.log("image",image);
    
    setIsDrawing(true);
    draw(e);
  };

  const handleMouseUp = () => {
    setIsDrawing(false);
    ctxRef.current?.beginPath();
  };

  const handleMouseMove = (e: React.MouseEvent<HTMLCanvasElement>) => {
    if (!isDrawing) return;
    draw(e);
  };

  const draw = (e: React.MouseEvent<HTMLCanvasElement>) => {
    const canvas = canvasRef.current;
    const ctx = ctxRef.current;
    if (!canvas || !ctx) return;

    const rect = canvas.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;

    ctx.strokeStyle = isEraseMode ? '#ffffff' : color;
    ctx.lineWidth = brushSize;
    ctx.lineCap = 'round';
    ctx.lineTo(x, y);
    ctx.stroke();
    ctx.beginPath();
    ctx.moveTo(x, y);
  };

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) setImage(URL.createObjectURL(file));
  };

  const clearCanvas = () => {
    const canvas = canvasRef.current;
    const ctx = ctxRef.current;
    if (canvas && ctx) {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      if (image) {
        const img = new Image();
        img.src = image;
        img.onload = () => {
          ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
        };
      }
    }
  };

  const toggleEraseMode = () => {
    setIsEraseMode(!isEraseMode);
  };

  const printCanvas = () => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const dataUrl = canvas.toDataURL('image/png');
    const printWindow = window.open('', '', 'height=500,width=500');
    printWindow?.document.write('<img src="' + dataUrl + '" />');
    printWindow?.document.close();
    printWindow?.print();
  };

  const downloadCanvas = () => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const dataUrl = canvas.toDataURL('image/png');
    const link = document.createElement('a');
    link.href = dataUrl;
    link.download = 'canvas-image.png';
    link.click();
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '16px', padding: '16px' }}>
      {/* Upload Image */}
      <Grid container spacing={2} justifyContent="center">
        <Grid item>
          <input
            type="file"
            accept="image/*"
            onChange={handleImageUpload}
            id="upload-button"
            style={{ display: 'none' }}
          />
          <label htmlFor="upload-button">
            <Button variant="contained" component="span" startIcon={<Upload />}>
              Upload Image
            </Button>
          </label>
        </Grid>
        <Grid item>
          <Button variant="contained" color={isEraseMode ? 'secondary' : 'primary'} onClick={toggleEraseMode}>
            {isEraseMode ? 'Switch to Paint' : 'Erase'}
          </Button>
        </Grid>
      </Grid>

      {/* Edit Colors Section with Icon */}
      <Box
        display="flex"
        flexDirection="column"
        alignItems="center"
        gap="8px"
        style={{ backgroundColor: '#f5f5f5', padding: '10px', borderRadius: '8px', width: '200px' }}
      >
        <Typography variant="body2" color="textPrimary" style={{ display: 'flex', alignItems: 'center' }}>
          <Palette style={{ marginRight: '8px' }} /> ערוך צבעים
        </Typography>
        <Grid container spacing={1} justifyContent="center">
          <Grid item>
            <Button
              style={{
                backgroundColor: color,
                width: '40px',
                height: '40px',
                borderRadius: '50%',
                boxShadow: '0 2px 5px rgba(0, 0, 0, 0.2)',
              }}
              onClick={() => setColor(color)}
            />
          </Grid>
          <Grid item>
            <input
              type="color"
              value={color}
              onChange={(e) => setColor(e.target.value)}
              style={{ width: '40px', height: '40px', borderRadius: '50%' }}
            />
          </Grid>
        </Grid>
      </Box>

      {/* Colors Picker */}
      <Grid container spacing={1} justifyContent="center">
        {[
          '#FF0000', '#00FF00', '#0000FF', '#FFFF00', '#FF00FF', '#00FFFF', '#000000', '#FFFFFF',
          '#FFA500', '#800080', '#008000', '#FFD700', '#A52A2A', '#D2691E', '#C71585', '#B0C4DE'
        ].map((colorOption) => (
          <Grid item key={colorOption}>
            <Button
              style={{
                backgroundColor: colorOption,
                width: '40px',
                height: '40px',
                borderRadius: '50%',
                boxShadow: '0 2px 5px rgba(0, 0, 0, 0.2)',
              }}
              onClick={() => setColor(colorOption)}
            />
          </Grid>
        ))}
      </Grid>

      {/* Brush Size Slider */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        <Slider
          value={brushSize}
          min={1}
          max={50}
          onChange={(e, newValue) => setBrushSize(newValue as number)}
          style={{ width: '200px' }}
        />
        <span>Brush size: {brushSize}</span>
      </div>

      {/* Canvas */}
      <canvas
        ref={canvasRef}
        style={{
          border: '1px solid #000',
          borderRadius: '8px',
          cursor: isEraseMode ? 'url(/eraser-cursor.png), auto' : 'crosshair',
          backgroundColor: '#f0f0f0',
        }}
        onMouseDown={handleMouseDown}
        onMouseUp={handleMouseUp}
        onMouseMove={handleMouseMove}
      ></canvas>

      {/* Clear Button */}
      <Button variant="contained" color="error" onClick={clearCanvas}>
        Clear
      </Button>

      {/* Print and Download Buttons in a row */}
      <div style={{ display: 'flex', gap: '8px' }}>
        <Button
          variant="contained"
          color="primary"
          onClick={printCanvas}
          startIcon={<Print />}
        >
          Print
        </Button>

        <Button
          variant="contained"
          color="primary"
          onClick={downloadCanvas}
          startIcon={<Print />}
        >
          Download
        </Button>
      </div>
    </div>
  );
};

export default PaintCanvas;
