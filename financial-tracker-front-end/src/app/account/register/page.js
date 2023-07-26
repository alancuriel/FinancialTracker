"use client"
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as Yup from 'yup';
import { motion } from "framer-motion";

import { Link } from '../../../components/Link';
import { Layout } from '../../../components/account/Layout';
import { userService} from '../../../services/user.service';

export default Register;

const icon = {
    hidden: {
      opacity: 0,
      pathLength: 0,
      fill: "rgba(6, 182, 213, 0)"
    },
    visible: {
      opacity: 1,
      pathLength: 1,
      fill: "rgba(6, 182, 213, 1)"
    }
  };

function Register() {
    const router = useRouter();

    // form validation rules 
    const validationSchema = Yup.object().shape({
        firstName: Yup.string()
            .required('First Name is required'),
        lastName: Yup.string()
            .required('Last Name is required'),
        email: Yup.string()
            .required('Email is required'),
        password: Yup.string()
            .required('Password is required')
            .min(6, 'Password must be at least 6 characters'),
        confirmPassword: Yup.string()
            .required('Password is required')
            .min(6, 'Password must be at least 6 characters')
    });
    const formOptions = { resolver: yupResolver(validationSchema) };

    // get functions to build form with useForm() hook
    const { register, handleSubmit, formState } = useForm(formOptions);
    const { errors } = formState;

    function onSubmit(user) {
        return userService.register(user)
            .then(() => {
                //alertService.success('Registration successful', { keepAfterRouteChange: true });
                router.push('login');
            })
            .catch(console.log("pop"))
            //.catch(alertService.error);
    }

    return (
        <Layout>
            <div class="max-w-sm rounded overflow-hidden">
            <div className="containersvg">
            
            <motion.svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="-12 -15 153.792 163.26"
      className="itemsvg  animate-blob"
    >
      

      <motion.path
        stroke= "#06b6d5"
        stroke-width= "2"
        stroke-linejoin= "round"
        stroke-linecap= "round"
        d="M21.45,103.07c24.32-.79,20.08-35.24,19.93-51C41.25,39,42.45,23.94,52,13.93s25-10.71,33.68.83c11.56,15.45,2,38.4,1.21,55.65-.46,9.57,1.61,19.84,8.19,27.12a27.2,27.2,0,0,0,10.84,7c4.6,1.75,10.31,2.74,13.92,6.33,9,9-8.51,7.78-13.14,7A43.64,43.64,0,0,1,76.16,96.8c-1-1.77-4.24-2.13-5.18,0-2.3,5.23-2.32,10.58-2.3,16.2s.3,13.1-5.39,16.12c-4.25,2.26-7.95-.23-8-5.1,0-3.53,1.42-6.91,2.76-10.1,2.53-6,4.24-11.47,1.56-17.75-.72-1.69-3.59-2-4.72-.61-7.75,9.56-18.58,17-30.85,19.26-4.14.75-9,1.14-13-.46a7.76,7.76,0,0,1-4.35-3.88c-1.74-3.77.43-9.65,5.41-8.31,3.73,1,5.32-4.78,1.59-5.79-8.15-2.2-15,5.48-13.42,13.45,1.29,6.72,7,10.38,13.38,11.28,17.25,2.42,35.06-8.41,45.51-21.31l-4.71-.61c2.08,4.88-.94,10.06-2.75,14.53-1.5,3.69-2.68,7.67-2.32,11.69.58,6.67,5.91,11.74,12.78,10.43,17.9-3.39,9.07-24.7,14-36H71a49.79,49.79,0,0,0,30.32,23c6.49,1.67,14.85,2.84,21.27.41,8.08-3.05,6.47-12.07,1.07-17.07-3.82-3.54-9.06-5-13.86-6.68C103.72,97.43,99,94.26,96,88.33c-5.27-10.56-2.86-22.83-1.17-33.95,1.6-10.45,3.4-21.46.83-31.91C93.21,12.42,85.51,3.11,75.14.67,62.75-2.25,50.31,4.72,43.59,15c-8.86,13.49-8.43,30.43-8,45.88.29,10.72,2.18,35.71-14.13,36.24-3.85.13-3.87,6.13,0,6Z"
        variants={icon}
        initial="hidden"
        animate="visible"
        transition={{
          default: { duration: 1.5, ease: "easeInOut" },
          fill: { duration: 1.5, ease: [1, 0, 0.8, 1] }
        }}
      />
      <motion.path
        stroke= "#06b6d5"
        stroke-width= "2"
        stroke-linejoin= "round"
        stroke-linecap= "round"
        d="M75.66,40.43c3.86,0,3.87-6,0-6s-3.87,6,0,6Z"
        variants={icon}
        initial="hidden"
        animate="visible"
        transition={{
          default: { duration: 1.5, ease: "easeInOut" },
          fill: { duration: 1.5, ease: [1, 0, 0.8, 1] }
        }}
      />
      <motion.path
        stroke= "#06b6d5"
        stroke-width= "2"
        stroke-linejoin= "round"
        stroke-linecap= "round"
        d="M60.06,53a9.34,9.34,0,0,0,10.41,1.35,3,3,0,1,0-3-5.18,3.09,3.09,0,0,1-3.15-.41,3.08,3.08,0,0,0-4.24,0,3,3,0,0,0,0,4.24Z"
        variants={icon}
        initial="hidden"
        animate="visible"
        transition={{
          default: { duration: 1.5, ease: "easeInOut" },
          fill: { duration: 1.5, ease: [1, 0, 0.8, 1] }
        }}
      />
      <motion.path
        stroke= "#06b6d5"
        stroke-width= "2"
        stroke-linejoin= "round"
        stroke-linecap= "round"
        d="M55.4,39.68c3.86,0,3.87-6,0-6s-3.87,6,0,6Z"
        variants={icon}
        initial="hidden"
        animate="visible"
        transition={{
          default: { duration: 1.5, ease: "easeInOut" },
          fill: { duration: 1.5, ease: [1, 0, 0.8, 1] }
        }}
      />
    </motion.svg>
    
    </div>
                        <div class="px-6 py-4 flex flex-col items-center">
                            <div class="font-bold text-white  text-xl align-middle ">Register</div>
                            <div className="flex-col">
                    <form onSubmit={handleSubmit(onSubmit)}>
                    <div className="form-group flex items-center my-4 justify-between">
                            
                            <input name="firstName" placeholder="First Name" class="rounded-full px-4 py-3 w-full" type="text" {...register('firstName')} className={`rounded-full px-4 py-3 w-full ${errors.firstName ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.firstName?.message}</div>
                        </div>
                        <div className="form-group flex items-center my-4 justify-between">
                            
                            <input name="lastName" placeholder="Last Name" class="rounded-full px-4 py-3 w-full" type="text" {...register('lastName')} className={`rounded-full px-4 py-3 w-full ${errors.lastName ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.lastName?.message}</div>
                        </div>
                        <div className="form-group flex items-center my-4 justify-between">
                            
                            <input name="email" placeholder="E-mail" class="rounded-full px-4 py-3 w-full" type="email" {...register('email')} className={`rounded-full px-4 py-3 w-full ${errors.username ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.username?.message}</div>
                        </div>
                        <div className="form-group flex items-center my-4 justify-between">
                            
                            <input name="password" placeholder="Password" class="rounded-full px-4 py-3 w-full" type="password" {...register('password')} className={`rounded-full px-4 py-3 w-full ${errors.password ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.password?.message}</div>
                        </div>
                        <div className="form-group flex items-center my-4 justify-between">
                            
                            <input name="confirmpassword" placeholder="Password" class="rounded-full px-4 py-3 w-full" type="password" {...register('confirmPassword')} className={`rounded-full px-4 py-3 w-full ${errors.confirmpassword ? 'is-invalid' : ''}`} />
                            <div className="invalid-feedback">{errors.confirmpassword?.message}</div>
                        </div>
                        <div class="flex flex-row items-center align-middle justify-around my-4">
                        <div>
                        <button disabled={formState.isSubmitting} class="bg-tangerine inline-block bg-gray-200 w-full rounded-full px-3 py-1 text-sm font-semibold text-gray-700 mr-2 mb-2">
                            {formState.isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                            Register
                        </button>
                        </div>
                        <div>
                        <button class="bg-tangerine inline-block bg-gray-200 rounded-full px-3 py-1 text-sm w-full font-semibold text-gray-700 mr-2 mb-2">
                        <Link href="/account/login">Cancel</Link>
                        </button>
                        </div>
                        </div>
                    </form>
                </div>
            </div>
            </div>
        </Layout>
    );
}