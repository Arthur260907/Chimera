# TODO: Fix Profile Page

## Backend Changes
- [x] Modify UsuariosController login endpoint to return email along with username
- [x] Add GET /api/Usuarios/profile endpoint to fetch user data by email (requires authentication, but since no JWT, use email from query or body)
- [ ] Add favorites functionality: Create Favorites table/model, controller, service for adding/removing favorites, and endpoint to get user's favorites

## Frontend Changes
- [x] Update header.js login to save email in localStorage
- [x] Update perfil.html to fetch profile data from API and populate name and email
- [x] Add sections to perfil.html: Settings, History, Favorites
- [x] Implement favorites display in perfil.html with carousel
- [x] Add basic footer content
- [ ] Make header filter links (Movies, Series) functional

## Testing
- [ ] Test login saves email
- [ ] Test profile page loads user data
- [ ] Test favorites section displays movies
