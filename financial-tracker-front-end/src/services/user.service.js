import { BehaviorSubject } from 'rxjs';
import { fetchWrapper } from '../helpers/fetch-wrapper';

const baseUrl = `${process.env.NEXT_PUBLIC_PUBLICAPI}/api/v1/authenticate`;
const userSubject = new BehaviorSubject((typeof window !== 'undefined') && JSON.parse(localStorage.getItem('user')));

export const userService = {
    user: userSubject.asObservable(),
    get userValue () { return userSubject.value },
    login,
    logout,
    register,
    getAll,
    getCategories,
    getById,
    uploadFile,
    update,
    delete: _delete
};

async function login(email, password) {
    

    const user = await fetchWrapper.post(`${baseUrl}/login`, { email, password });
    // publish user to subscribers and store in local storage to stay logged in between page refreshes
    console.log(user)
    userSubject.next(user);
    localStorage.setItem('user', JSON.stringify(user));
    return user;
}

function logout() {
    // remove user from local storage, publish null to user subscribers and redirect to login page
    localStorage.removeItem('user');
    userSubject.next(null);
    
}

function register(user) {
    return fetchWrapper.post(`${baseUrl}/register`, user);
}

function getAll() {
    return fetchWrapper.get(`${process.env.NEXT_PUBLIC_PUBLICAPI}/v1/financial/recent-transactions/31`);
}

function  getCategories()  {
    let cachedCats = (typeof window !== 'undefined') && localStorage.getItem('categories');

    if (cachedCats !== null) {
        return new Promise(function(resolve, reject) {
            resolve(JSON.parse(cachedCats));
          });
    }

    return fetchWrapper.get(`${process.env.NEXT_PUBLIC_PUBLICAPI}/v1/financial/categories`);
}

function uploadFile(file) {
    const url = `${process.env.NEXT_PUBLIC_PUBLICAPI}/copilotupload`
    return fetchWrapper.postFormFile(url, file, file.name);
}

function getById(id) {
    return fetchWrapper.get(`${baseUrl}/${id}`);
}

async function update(id, params) {
    const x = await fetchWrapper.put(`${baseUrl}/${id}`, params);
    // update stored user if the logged in user updated their own record
    if (id === userSubject.value.id) {
        // update local storage
        const user = { ...userSubject.value, ...params };
        localStorage.setItem('user', JSON.stringify(user));

        // publish updated user to subscribers
        userSubject.next(user);
    }
    return x;
}

// prefixed with underscored because delete is a reserved word in javascript
function _delete(id) {
    return fetchWrapper.delete(`${baseUrl}/${id}`);
}