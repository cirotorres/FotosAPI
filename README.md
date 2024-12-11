# Documentação da API - FotosAPI
API responsável em armazenar, processar a qualidade e resgatar a imagem relacionada ao Objeto cadastrado no Banco de dados.

Te possibilita fazer o Upload da imagem definindo em porcentagem a qualidade que será armazenada. 
Possibilidade de criação de Thumbnail para a imagem carregada, sendo criada em menor proporção pré-definida. 

## Endpoints

### 1. Geração de Token
- **URL**: `/api/Auth/generate-token`
- **Método**: `GET`
- **Descrição**: Gera um token Bearer JWT com a inclusão de informações como "uploadedBy" e "applicationId" .
- **Resposta** (200 OK):
 ```json
  {
  "token": "XXXXXXXXXXXX"
  }
 ```
  
### 2. Upload de Foto
- **URL**: `/photos/upload`
- **Método**: `POST`
- **Descrição**: Faz o upload de uma nova foto, criando uma pasta de acordo com o nome do "applicationId".
- **Cabeçalhos**:
  - `Authorization: Bearer {token}`
- **Request Body** (multipart/form-data):
  - `Picture` (file): Arquivo da imagem.
  - `Title` (string): Título da foto.
  - `Quality` (int): Qualidade da imagem (1-100).
  - `Thumbnail` (bool): Se `true`, cria uma thumbnail.
- **Resposta** (200 OK) exemplo:
  ```json
  {
    "id": 123,
    "picturePath": "Storage\\Aplicacao\\nome_arquivo.jpg",
    "title": "Tìtulo da foto",
    "width": 2372,
    "height": 1680,
    "uploadedAt": "2024-11-07T23:36:14.6796011-03:00",
    "uploadedBy": "Nome Usuário",
    "applicationId": "Nome da Aplicação",
    "isThumbnail": true,
    "thumbPath": "Storage\\Aplicacao\\thumbnails\\nome_arquivo.jpg"
  }
  ```

### 3. Listagem de objetos cadastrados
- **URL**: `/photos/AllList`
- **Método**: `GET`
- **Descrição**: Retorna todos os objetos cadastrados.
- **Resposta** (200 OK):
  ```json
  [
    {
      "id": 68,
      "title": "teste4",
      "uploadedBy": "Ciro Torres",
      "applicationId": "APIFotos",
      "uploadedAt": "2024-11-06T16:50:51.211503-03:00",
      "thumbPath": "http://localhost:0000/Storage/Aplicacao/thumbnails/nome_arquivo.jpg"
    },
    {
      "id": 69,
      "title": "teste5",
      "uploadedBy": "Ciro Torres",
      "applicationId": "APIFotos",
      "uploadedAt": "2024-11-06T16:54:09.516976-03:00",
      "thumbPath": "http://localhost:0000/Storage/Aplicacao/thumbnails/nome_arquivo2.jpg"
    }
  ]
  ```

### 4. Busca dados do objeto por ID
- **URL**: `/photos/find/{id}`
- **Método**: `GET`
- **Cabeçalhos**:
  - `Authorization: Bearer {token}`
- **Descrição**: Retorna dados específicos do objeto com base no ID.
- **Parâmetros**:
  - `id` (string): O ID da foto.
- **Resposta** (200 OK) exemplo:
  ```json
  {
    "id": 123,
    "picturePath": "Storage\\APIFotos\\exemplo.jpg",
    "title": "wallpaperpaisagem",
    "width": 1920,
    "height": 1080,
    "uploadedAt": "2024-11-06T17:06:37.211421-03:00",
    "uploadedBy": "Ciro Torres",
    "applicationId": "APIFotos",
    "isThumbnail": true,
    "thumbPath": "Storage\\Aplicacao\\thumbnails\\nome_arquivo.jpg"
  }
  ```

### 5. Busca a Foto relacionada ao objeto por ID
- **URL**: `/photos/view/{id}`
- **Método**: `GET`
- **Cabeçalhos**:
  - `Authorization: Bearer {token}`
- **Descrição**: Retorna a foto específica do objeto com base no ID.
- **Parâmetros**:
  - `id` (string): O ID da foto.
- **Resposta** (200 OK) exemplo:
  ```
    Foto_do_Objeto.jpg
  ```
- **Response headers**:
  ```
  cache-control: public,max-age=300 
  content-length: 34294 
  content-type: image/jpg 
  date: Fri,08 Nov 2024 02:36:57 GMT 
  photo-creationdate: 2024-11-06T17:06:37.2114210-03:00 
  photo-height: 1080 
  photo-title: wallpaperpaisagem 
  photo-width: 1920 
  server: Kestrel 
  ```
  
### 6. Busca a Thumbnail relacionada ao objeto por ID
- **URL**: `/photos/thumbnail/{id}`
- **Método**: `GET`
- **Cabeçalhos**:
  - `Authorization: Bearer {token}`
- **Descrição**: Retorna a thumbnail específica do objeto com base no ID.
- **Parâmetros**:
  - `id` (string): O ID da foto.
- **Resposta** (200 OK) exemplo:
  ```
    Thumbnail_do_Objeto.jpg
  ```
- **Response headers**:
  ```
  content-length: 6743 
  content-type: image/jpg 
  date: Wed,11 Dec 2024 02:56:51 GMT 
  server: Kestrel

  ```

### 7. Deletar Foto
- **URL**: `/photos/delete/{id}`
- **Método**: `DELETE`
- **Cabeçalhos**:
  - `Authorization: Bearer {token}`
- **Descrição**: Deleta todo o objeto e seus registros, com base no ID.
- **Parâmetros**:
  - `id` (string): O ID da foto.
- **Resposta** (200 OK):
 ```
  Response body: Objeto, Foto e miniatura deletados com sucesso.
 ```