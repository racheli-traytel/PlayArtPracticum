import { Component } from '@angular/core';
import { HttpClient, HttpEventType, HttpEvent, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-file-upload',
  standalone: true,
  imports: [HttpClientModule],
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent {
  selectedFile: File | null = null;
  uploadProgress = 0;
  uploadSuccess = false;
  uploadError: string | null = null;
  filePreview: string | null = null; // תצוגה מקדימה של הקובץ
  isImage: boolean = false; // האם מדובר בתמונה או לא

  constructor(private http: HttpClient) {}

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
    if (this.selectedFile) {
      this.resetUploadStatus(); // איפוס סטטוס ההעלאה
      this.previewFile(this.selectedFile); // הצגת תצוגה מקדימה של הקובץ
    }
  }

  // הצגת תצוגה מקדימה של הקובץ
  previewFile(file: File): void {
    const reader = new FileReader();

    // בדיקה אם מדובר בתמונה
    if (file.type.startsWith('image/')) {
      this.isImage = true;
      reader.onload = () => {
        this.filePreview = reader.result as string;
      };
      reader.readAsDataURL(file); // קריאת הקובץ כ-URL של תמונה
    } else if (file.type === 'text/plain') {
      this.isImage = false;
      reader.onload = () => {
        this.filePreview = reader.result as string;
      };
      reader.readAsText(file); // קריאת קובץ טקסט
    } else {
      this.isImage = false;
      this.filePreview = null; // אם מדובר בקובץ שלא ניתן להציג אותו בתצוגה מקדימה
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      return;
    }

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.http.post('YOUR_UPLOAD_API_ENDPOINT', formData, {
      reportProgress: true,
      observe: 'events'
    }).subscribe({
      next: (event: HttpEvent<any>) => {
        if (event.type === HttpEventType.UploadProgress) {
          this.uploadProgress = Math.round((event.loaded / (event.total || 1)) * 100);
        } else if (event.type === HttpEventType.Response) {
          this.uploadSuccess = true;
        }
      },
      error: (error) => {
        this.uploadError = error.message;
      },
      complete: () => {
        console.log('העלאה הסתיימה');
      }
    });
  }

  private resetUploadStatus(): void {
    this.uploadProgress = 0;
    this.uploadSuccess = false;
    this.uploadError = null;
  }
}