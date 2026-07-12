// Initial data for the application
const initialData = {
    locations: [
        { id: 1, name: "منوف", address: "شارع بورسعيد" },
        { id: 2, name: "السادات", address: "المنطقة الرابعة" }
    ],
    grades: [
        { id: 1, name: "الصف الأول الثانوي" },
        { id: 2, name: "الصف الثاني الثانوي" },
        { id: 3, name: "الصف الثالث الثانوي" }
    ],
    groups: [
        { id: 1, locationId: 1, gradeId: 1, name: "مجموعة السبت", time: "10:00 AM" },
        { id: 2, locationId: 1, gradeId: 2, name: "مجموعة الأحد", time: "04:00 PM" },
        { id: 3, locationId: 2, gradeId: 3, name: "مجموعة الثلاثاء", time: "12:00 PM" }
    ],
    students: []
};

// Function to initialize data in localStorage if not exists
function initApp() {
    if (!localStorage.getItem('teacher_app_data')) {
        localStorage.setItem('teacher_app_data', JSON.stringify(initialData));
    }
}

initApp();

function getAppData() {
    return JSON.parse(localStorage.getItem('teacher_app_data'));
}

function saveAppData(data) {
    localStorage.setItem('teacher_app_data', JSON.stringify(data));
}
