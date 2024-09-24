const API_URL = 'https://localhost:7195/api/Post'
const postForm = document.getElementById('post-form');
const postsList = document.getElementById('posts-list');


fetchPosts();

// Fetch posts from API
async function fetchPosts() {
  try {
    const response = await fetch(API_URL);
    const posts = await response.json();

    displayPosts(posts);
  } catch (error) {
    console.error('Error fetching posts:', error);
  }
}

function displayPosts(posts) {
    postsList.innerHTML = '';

    posts.forEach(
        post => {
            const postElement = document.createElement('div');
            postElement.className = 'post';
            postElement.innerHTML = `
                <h3>${post.title}</h3>
                <p>${post.content}</p>
                <p>Author: ${post.author}</p>
                <p>Community: ${post.community}</p>
                <button onclick="edit-post(${post.id})">Edit</button>
                <button onclick="delete-post(${post.id})">Delete</button>
            `;

            postsList.appendChild(postElement);
        });
}

// Handle form submission (create/update post)
postForm.addEventListener(
    'submit', async (e) => {
        e.preventDefault();
        const postId = document.getElementById('post-id').value;
        const post = {
            title: document.getElementById('title').value,
            content: document.getElementById('content').value,
            author: document.getElementById('author-name').value,
            community: document.getElementById('community-name').value
        };

        try {
            const url = API_URL;
            const method = 'POST';
            const response = await fetch(url, {
                method: method,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(post)
            })
        } 
        catch (error) {
            console.error('Error: ', error);
        }
    }
);