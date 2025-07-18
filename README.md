# WAV to MP3 Converter Azure Function

This repository contains a C# Azure Function that converts uploaded WAV audio files to MP3 format using NAudio and LAME.

## Features

- Accepts WAV files via HTTP POST
- Converts WAV to MP3 in-memory
- Returns the MP3 file for download

## How to Use

1. **Deploy to Azure Functions**  
   - Clone this repository.
   - Deploy the project to your Azure Functions environment.

2. **Send a Request**  
   - Make a POST request to the function endpoint.
   - Use `multipart/form-data` with a form field named `file` containing your `.wav` file.

   Example using `curl`:

   curl -X POST https://<your-function-url>/api/ConvertWavToMp3
-F "file=@your-audio.wav" --output output.mp3

3. **Get the Result**  
   - The response will be an MP3 file converted from your WAV input.

## Requirements

- .NET 6 or later
- Azure Functions runtime
- NAudio and NAudio.Lame NuGet packages

## License

This project is open source and available under the MIT License.
